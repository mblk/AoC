package main

import (
	"bufio"
	"bytes"
	"fmt"
	"log"
	"os"
	"strconv"
)

const (
    Left uint8 = iota
    Right
)

const (
    North uint8 = iota
    East
    South
    West
)

type Instruction struct {
    Direction uint8
    Distance  uint32
}

type Position struct {
    X int
    Y int
}

func main() {
    fmt.Println("Hello!")

    if len(os.Args) != 2 {
        log.Fatal("Invalid arguments")
    }

    instructions := ParseInstructions(os.Args[1])

    part1 := FollowInstructions(instructions, false)
    part2 := FollowInstructions(instructions, true)

    fmt.Printf("Part1: %v (%v)\n", part1.CalculateBlocks(), part1)
    fmt.Printf("Part2: %v (%v)\n", part2.CalculateBlocks(), part2)
}

func AbsInt(value int) int {
    if value < 0 {
        return -value
    }
    return value;
}

func (p Position) CalculateBlocks() int {   
    return AbsInt(p.X) + AbsInt(p.Y)
}

func FollowInstructions(instructions []Instruction, stopIfAlreadyVisited bool) Position {
    position := Position{}
    heading := North
    visited := make(map[Position]bool)

    for _, instr := range instructions {
        switch instr.Direction {
            case Left: heading = (heading - 1) % 4
            case Right: heading = (heading + 1) % 4
        }
        for i:=uint32(0); i<instr.Distance; i++ {
            position = position.Move(heading)
            if stopIfAlreadyVisited && visited[position] {
                return position
            }
            visited[position] = true
        }
    }

    return position
}

func (p Position) Move(heading uint8) Position {
    // Passed by value. Modify and return.
    switch heading {
        case North: p.Y ++
        case South: p.Y --
        case East: p.X ++
        case West: p.X --
    }
    return p
}

func ParseInstructions(fileName string) []Instruction {
    file, err := os.Open(fileName)
    if err != nil {
        log.Fatal(err)
    }
    defer file.Close()

    var instructions []Instruction
    scanner := bufio.NewScanner(file)
    scanner.Split(ScanComma)
    for scanner.Scan() {
        line := scanner.Text()
        instruction := ParseInstruction(line)
        instructions = append(instructions, instruction)
    }

    if err := scanner.Err(); err != nil {
        log.Fatal(err)
    }

    return instructions
}

func ScanComma(data []byte, atEOF bool) (advance int, token []byte, err error) {
    const trimChars = " \n\r"
    if atEOF && len(data) == 0 {
        return 0, nil, nil
    }
    if idx := bytes.IndexByte(data, ','); idx > 0 {
        return idx + 1, bytes.Trim(data[0:idx], trimChars), nil
    }
    if atEOF {
        return len(data), bytes.Trim(data, trimChars), nil
    }
    return 0, nil, nil
}

func ParseInstruction(line string) Instruction {
    var direction uint8
    switch(line[0]) {
        case 'L': direction = Left
        case 'R': direction = Right
        default: log.Fatalf("Invalid instruction '%v'", line)
    }

    var distance uint32
    if value, err := strconv.ParseUint(line[1:], 10, 32); err == nil {
        distance = uint32(value)
    } else {
        log.Fatal("Failed to parse distance")
    }
    
    return Instruction{
        Direction: direction,
        Distance: distance,
    }
}

