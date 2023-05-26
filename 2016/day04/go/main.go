package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"sort"
	"strconv"
	"strings"
)

type Room struct {
    Name string
    SectorId int
    Checksum string
}

func main() {
    fmt.Println("Hello!")

    rooms := ParseRoomsFile(os.Args[1])

    correctRoomSectorIdSum := 0
    numCorrectRooms := 0

    for _, room := range rooms {
        if room.ChecksumIsCorrect() {
            correctRoomSectorIdSum += room.SectorId
            numCorrectRooms++
            //fmt.Printf("Correct: %v (%v)\n", room.GetDecryptedName(), room.SectorId)
        }
    }

    fmt.Printf("Total rooms: %d\n", len(rooms))
    fmt.Printf("Correct rooms: %d\n", numCorrectRooms)
    fmt.Printf("Sum of correct room's sector-IDs: %d\n", correctRoomSectorIdSum)

    for _, room := range rooms {
        if !room.ChecksumIsCorrect() {
            continue
        }
        decryptedName := room.GetDecryptedName()
        if !strings.Contains(decryptedName, "northpole") {
            continue
        }
        fmt.Printf("Found '%v' in sector %d\n", decryptedName, room.SectorId)
    }
}

func (r *Room) GetDecryptedName() string {

    decryptedName := ""

    for _, char := range r.Name {
        if char == '-' {
            decryptedName += " "
        } else {
            shiftedChar := char
            for i:=0; i<r.SectorId; i++ {
                if shiftedChar == 'z' {
                    shiftedChar = 'a'
                } else {
                    shiftedChar++
                }
            }
            decryptedName += string(shiftedChar)
        }
    }
    return decryptedName
}

func (r *Room) ChecksumIsCorrect() bool {
    return CalculateRoomChecksum(r.Name) == r.Checksum
}

func CalculateRoomChecksum(name string) string {

    counts := make(map[rune]int)

    for _, c := range name {
        if c == '-' { continue }
        counts[c]++
    }

    keys := make([]rune, 0, len(counts))
    for key := range counts {
        keys = append(keys, key)
    }

    sort.SliceStable(keys, func(i, j int) bool {
        key1 := keys[i]
        key2 := keys[j]
        if (counts[key1] == counts[key2]) {
            return key1 < key2
        } else {
            return counts[key1] > counts[key2]
        }
    })

    cs := ""
    for i:=0; i<5 && i<len(keys); i++ {
        cs += string(keys[i])
    }
    return cs;
}

func ParseRoomsFile(fileName string) []Room {
    var rooms []Room

    ReadFileLineByLine(fileName, func(line string) {
        rooms = append(rooms, ParseRoomLine(line))
    })

    return rooms
}

func ParseRoomLine(line string) Room {

    sep1 := strings.LastIndex(line, "-")
    sep2 := strings.Index(line, "[")
    sep3 := strings.Index(line, "]")

    if sep1 < 0 || sep2 < sep1 || sep3 < sep2 {
        log.Fatal("Invalid line")
    }

    name := line[:sep1]
    idStr := line[sep1+1:sep2]
    checksum := line[sep2+1:sep3]
 
    id, err := strconv.ParseInt(idStr, 10, 0)

    if err != nil {
        log.Fatal(err)
    }

    return Room{
        name, int(id), checksum,
    }
}

func ReadFileLineByLine(fileName string, fn func(string)) {
    file, err := os.Open(fileName)
    if err != nil {
        log.Fatal(err)
    }
    defer file.Close()

    scanner := bufio.NewScanner(file)
    for scanner.Scan() {
        line := scanner.Text()
        fn(line)
    }
    if err := scanner.Err(); err != nil {
        log.Fatal(err)
    }
}


