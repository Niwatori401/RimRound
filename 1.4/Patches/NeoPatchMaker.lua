function input_loop()
    local flag_r = false
    local flag_n = false
    local flag_l = false
    local number

    repeat
        local valid_input = true

        io.write("Enter your input (e.g. 1r, 2rn, 3n): ")
        local input = io.read()

        if input:find("r") then
          flag_r = true
          print("Will repeat patch maker process")
        end

        if input:find("n") then
          flag_n = true
          print("Alt new line (\\r\\n) selected")
        end

        if input:find("l") then
          flag_l = true
          print("Linux selected")
        end

        number = tonumber(input:match("%d+"))

        if number == 1 then
        print("Option 1 selected.")
        elseif number == 2 then
        print("Option 2 selected.")
        elseif number == 3 then
        print("Option 3 selected.")
        else
        print("Invalid input.")
        valid_input = false
        end
    until valid_input

    return number, flag_r, flag_n, flag_l
end


function delete_files_linux(directoriesToWipe)
  local files = {}
  for _, directory in pairs(directoriesToWipe) do
      local filePattern = directory .. ".*%.xml"
      local dirCmd = string.format("%s %q", "find", directory)
      local directoryFiles = io.popen(dirCmd):lines()
      for file in directoryFiles do
          if file:match(filePattern) then
          table.insert(files, file)
          end
      end
  end

  for _, file in pairs(files) do
  local success, err = os.remove(file)
      if not success then
          print("Failed to delete file: " .. file)
          if err then
              print("Error message: " .. err)
          end
      end
  end
end

function delete_files_windows(directoriesToWipe)
  local files = {}
  for _, directory in ipairs(directoriesToWipe) do
      local dirCmd = "cmd.exe /c dir /b /s /a-d \"" .. directory .. "\""
      local directoryFiles = io.popen(dirCmd):lines()
      
      for file in directoryFiles do
          table.insert(files, file)
      end
  end

  for _, file in pairs(files) do
      local success, err = os.remove(file)
          if not success then
              print("Failed to delete file: " .. file)
          if err then
              print("Error message: " .. err)
          end
      end
  end
end

function assemble_patches_and_write_to_file(basePath)
  local subpatches = make_all_subpatches(basePath)

  local basePatchFirstHalf = {}
  local basePatchSecondHalf = {}

  local foundBreak = false
  for line in io.lines(basePath.. "Patches/PatchMaker/FindModTemplate.~xml") do
      if string.find(line, "INSERTION_POINT") then
          foundBreak = true
      elseif not foundBreak then
          table.insert(basePatchFirstHalf, line)
      else
          table.insert(basePatchSecondHalf, line)
      end
  end

  for modname, patches in pairs(subpatches) do
      local modnameforfilename = string.gsub(modname, "[:<>\",\\/|?*]", "")
      write_patchtable(basePath .. "Patches/GeneratedPatches/".. modnameforfilename ..".xml", basePatchFirstHalf, patches, basePatchSecondHalf, modname)
  end

end


function write_patchtable(filename, basefirsthalf, innerpatch, baselasthalf, modname)
  file = io.open(filename, "w+")
  io.output(file)

  for i, line in pairs(basefirsthalf) do
      line = string.gsub(line, "MOD_NAME", modname)
      io.write(line .. "\n")
  end

  for i, patch in pairs(innerpatch) do
      for j, p in pairs(patch) do
          for _, line in pairs(p) do
              io.write(line .. "\n")
          end
      end
  end

  for i, line in pairs(baselasthalf) do
      io.write(line .. "\n")
  end

  io.close(file)
  io.output(io.stdout)
end


--Does not write files
function make_all_subpatches(basePath)
  local allPatchesByMod = {}
  local patchesData = get_patchdata(basePath)

  for _, patchData in pairs(patchesData) do
      local result = make_all_patches_for_patch_data(patchData, basePath)

      for modNameKey, resultData in pairs(result) do
          if allPatchesByMod[modNameKey] == nil then
              allPatchesByMod[modNameKey] = {}
          end

          for _, localResult in pairs(resultData) do
              table.insert(allPatchesByMod[modNameKey], localResult)
          end
      end
  end

  return allPatchesByMod
end

--[[
  Returns a table keyed by mod name. Each entry stores one composite patch for each mod
]]
function make_all_patches_for_patch_data(patchData, basePath)
  local patchdataByMod = {}

  local csvFileLocation = basePath .. "Patches/PatchMaker/" .. patchData[2] .. ".csv"
  local templateFileLocation = basePath .. "Patches/PatchMaker/" .. patchData[1] .. ".~xml"

  local unsortedTokens = read_tokens_from_file(csvFileLocation)
  local valuesToReplace = unsortedTokens[1]
  local csvTokensByMod = group_csv_patches_by_mod(unsortedTokens)


  for mod, patchesForMod in pairs(csvTokensByMod) do
      local modResult = {}
      for _, csvRow in pairs(patchesForMod) do
          local singlePatchEntryResult = {}

          for line in io.lines(templateFileLocation) do
              local lineAfterReplacement = line
              for i, valueToReplace in pairs(valuesToReplace) do
                  lineAfterReplacement = string.gsub(lineAfterReplacement, valueToReplace, csvRow[i])
              end
              table.insert(singlePatchEntryResult, lineAfterReplacement)
          end
          table.insert(modResult, singlePatchEntryResult)
      end

      if patchdataByMod[mod] == nil then
          patchdataByMod[mod] = {}
      end

      table.insert(patchdataByMod[mod], modResult)
  end

  return patchdataByMod
end

function condense_patches_for_mod(modResult)
  --[[ each entry in this table is a table containing condensable patches
      Example structure
      {
          {{individual patch} {individual patch}},
          {other group}
      }
  ]]

  local patchesToBeCondensed = {}

  for _, individualPatch in pairs(modResult) do
      local foundCondensableGroup = false
      for j, patchGroup in pairs(patchesToBeCondensed) do
          for k, lineFromPatchGroup in pairs(patchGroup[1]) do --Any member would work
              if lineFromPatchGroup ~= individualPatch[k] then
                  if not string.find(lineFromPatchGroup, "<!--{D}-->") then
                      print(lineFromPatchGroup)
                      print("------------")
                      print(individualPatch[k])
                      foundCondensableGroup = false
                      break
                  end
              end

              foundCondensableGroup = true
          end

          if foundCondensableGroup then
              print("merged patch")
              table.insert(patchesToBeCondensed[j], individualPatch)
              break
          end
      end

      if not foundCondensableGroup then table.insert(patchesToBeCondensed, {individualPatch}) end
  end

  local totalResult = {}

  --This is a table of lines representing a complete condensed patch for this group
  local groupResult = {}
  for i, group in pairs(patchesToBeCondensed) do

      local templateGroup = {}

      local individualGroup = {}
      for _, line in pairs(group[1]) do
          if not string.find(line, "<!--{D}-->") then
              table.insert(individualGroup, line)
          else
              if (#individualGroup > 0) then table.insert(templateGroup, individualGroup) end
              individualGroup = {}
          end
      end

      individualGroup = {}
      local duplicationGroup = {}

      local groupIndex = 1
      local lineIndex = 0
      for _, _ in pairs(templateGroup) do
          for _, patch in pairs(group) do
              local offset = #templateGroup[groupIndex] + 1
              table.insert(individualGroup, patch[lineIndex + offset])
          end
          table.insert(duplicationGroup, individualGroup)
          individualGroup = {}
          lineIndex = lineIndex + offset
      end

      for i = 1, #duplicationGroup, 1 do
          for _, line in pairs(templateGroup[i]) do
              table.insert(groupResult, line)
          end

          for _, line in pairs(duplicationGroup[i]) do
              table.insert(groupResult, line)
          end
      end

      for _, line in pairs(templateGroup[#templateGroup]) do
          table.insert(groupResult, line)
      end

      table.insert(totalResult, groupResult)
      groupResult = {}
  end


  return totalResult
end

function explode_table(table, level)
  level = level or 0
  for _, entry in pairs(table) do
      for i = 0, level, 1 do io.write('+') end
      if type(entry) == 'table' then
          explode_table(entry, level + 1)
      else
          io.write(entry)
      end
      io.write('\n')
  end
end

function read_tokens_from_file(filename)
  local tokens = {}
  local numberOfColumns = 0
  local columnsCalculated = false
  for line in io.lines(filename) do
    local line_tokens = {}
    for token in string.gmatch(line, "[^,]+") do
      table.insert(line_tokens, token)
      if not columnsCalculated then numberOfColumns = numberOfColumns + 1 end
    end
    columnsCalculated = true
    table.insert(tokens, line_tokens)
  end
  return tokens, numberOfColumns
end


function group_csv_patches_by_mod(csvTokens)
  --Keyed by mod name
  local patchesByMod = {}

  for i, row in pairs(csvTokens) do
      repeat
          if i == 1 or row[1] == nil then break end
          local modName = row[1]
          if patchesByMod[modName] == nil then
              patchesByMod[modName] = {}
          end
          table.insert(patchesByMod[modName], row)
      until true
  end

  return patchesByMod
end


function get_base_path()
  print("Please input the RimRound install directory. For example:")
  print("C:/Program Files (x86)/Steam/steamapps/common/RimWorld/Mods/RimRound/1.4/")
  print("Make sure to include the last slash at the end.")

  local userInput = io.read()

  return userInput
end

--Works
function get_patchdata(baseDirectory)
  local aPatchData = {}
  table.insert(aPatchData, {"RimRound_AdjustAlignWithHeadTag",                    "alignWithHeadPatches"                       })
  table.insert(aPatchData, {"RimRound_AddComps",                                  "compsPatches",                              })
  table.insert(aPatchData, {"RimRound_AlignBodyPart",                             "bodyPartAlignmentPatches",                  })
  table.insert(aPatchData, {"RimRound_RemoveBodySpecificOffsets",                 "removeBodySpecificOffsetsPatches",          })
  table.insert(aPatchData, {"RimRound_BodyTypeSpecificAlignmentPatch",            "bodyTypeSpecificAlignmentPatch",            })
  table.insert(aPatchData, {"RimRound_RemoveGeneralOffset",                       "removeGeneralOffsetPatch",                  })
  table.insert(aPatchData, {"RimRound_SetBodyAddonDrawsize",                      "setBodyAddonDrawsize",                      })
  table.insert(aPatchData, {"RimRound_RemoveScaleWithBodyDrawsizeTag",            "removeScaleWithBodyDrawsizeTag",            })
  table.insert(aPatchData, {"RimRound_RemoveBodySpecificPortraitOffset",          "removeBodySpecificPortraitOffsetsPatches",  })
  table.insert(aPatchData, {"RimRound_RemoveGeneralHeadOffset",                   "removeGeneralHeadOffset",                   })
  table.insert(aPatchData, {"RimRound_RemoveSpecificGraphicPathHeadOffset",       "removeSpecificGraphicPathHeadOffset",       })
  table.insert(aPatchData, {"RimRound_RemoveDefaultOffset",                       "removeDefaultOffset",                       })
  table.insert(aPatchData, {"RimRound_AlignBodyPartAbstractDef",                  "bodyPartAlignmentPatchesAbstract",          })
  table.insert(aPatchData, {"RimRound_BodyTypeSpecificAlignmentPatchAbstractDef", "bodyTypeSpecificAlignmentPatchAbstract",    })
  table.insert(aPatchData, {"RimRound_RemoveScaleWithBodyDrawsizeTagAbstractDef", "removeScaleWithBodyDrawsizeTagAbstract",    })
  table.insert(aPatchData, {"RimRound_AddNutritionDensity",                       "setNutritionDensity",                       })
  table.insert(aPatchData, {"RimRound_AlterMaxToIngestAtOnceTag",                 "alterMaxToIngestTag",                       })
  table.insert(aPatchData, {"RimRound_AdjustAlignWithHeadTag",                    "alignWithHeadPatches",                      })

  return aPatchData
end


repeat
  local patcherMode, shouldRepeat, _, usingLinux = input_loop()

  sBasePath = get_base_path()

  if patcherMode == 1 or patcherMode == 3 then
    print("Wiping old patches")
    if usingLinux then
      delete_files_linux({sBasePath .. "/Patches/GeneratedPatches/"})
    else
      delete_files_windows({sBasePath .. "/Patches/GeneratedPatches/"})
    end
  end
  if patcherMode == 1 or patcherMode == 2 then
    --Make patches
    assemble_patches_and_write_to_file(sBasePath)
  end
until not shouldRepeat
