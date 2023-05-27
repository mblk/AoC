package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"sort"
)

func main() {
    fmt.Println("Hello!")

    if len(os.Args) != 2 {
        log.Fatal("Missing argument")
    }

    input := ReadInputFile(os.Args[1])

    message1 := FindMessage(input, true)
    message2 := FindMessage(input, false)

    fmt.Printf("Part1: '%v'\n", message1)
    fmt.Printf("Part2: '%v'\n", message2)
}


func FindMessage(input []string, useMostCommon bool) string {

    messageLength := len(input[0])
    message := ""

    for position:=0; position<messageLength; position++ {
        // Count character usage in this column.
        charCounts := make(map[rune]int)
        for _, word := range input {
            char := rune(word[position])
            charCounts[char]++
        }

        // Sort by count.
        keys := make([]rune, 0, len(charCounts))
        for k := range charCounts {
            keys = append(keys, k)
        }
        sort.SliceStable(keys, func(i, j int) bool {
            return charCounts[keys[i]] > charCounts[keys[j]]
        })

        // Pick most/least common.
        if useMostCommon {
            message += string(keys[0])
        } else {
            message += string(keys[len(keys)-1])
        }
    }

    return message
}

func ReadInputFile(fileName string) []string {
    var input []string

    ReadFileLineByLine(fileName, func(line string) {
        input = append(input, line)
    })

    return input
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



