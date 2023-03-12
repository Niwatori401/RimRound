import math
minorVersion = int(input("Minor Version? "))
revision = int(input("Revision? "))

minutes = ((revision * 2) / 60) % 60
hours = math.floor(((revision * 2) / 60) / 60)

yearsPassed = 0
i = 1
while minorVersion > 365:
    if i % 4 == 0:
        minorVersion -= 366
        yearsPassed += 1
    else:
        minorVersion -= 365
        yearsPassed += 1
    i += 1

daysInMonths = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
monthNumber = 1
j = 0
while minorVersion > daysInMonths[j]:
    minorVersion -= daysInMonths[j]
    monthNumber += 1
    j += 1

print(f"Build was made on {monthNumber}/{minorVersion} at {hours}:{math.floor(minutes)}")
input()


