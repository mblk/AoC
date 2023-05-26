package main

import (
    "fmt"
    "os"
    "bufio"
    "log"
    "math"
)

func main() {
    fmt.Println("Hello!")

    if len(os.Args) != 2 {
        log.Fatal("Missing input file")
    }

    instructions := ParseInstructions(os.Args[1])

    code1 := FollowInstructions(instructions, 1, 1, CheckPosition1, GetDigit1)
    code2 := FollowInstructions(instructions, 0, 2, CheckPosition2, GetDigit2)

    fmt.Printf("Part1: %X\n", code1)
    fmt.Printf("Part2: %X\n", code2)
}

func CheckPosition1(x, y int) bool {
    return 0 <= x && x <= 2 &&
           0 <= y && y <= 2
}

func CheckPosition2(x, y int) bool {
    mx := x - 2
    my := y - 2
    dist := math.Sqrt((float64)(mx*mx + my*my))
    return dist < 2.01
}

func GetDigit1(x, y int) int {
    return y*3 + x + 1
}

func GetDigit2(x, y int) int {
    offsets := []int{ -1, 1, 5, 9, 11 } // there's propably a smarter way to do this :D
    return offsets[y] + x
}

type CheckPositionFunc func(int, int) bool
type GetDigitFunc func(int, int) int

func FollowInstructions(instructions []string, x0 int, y0 int, check CheckPositionFunc, getDigit GetDigitFunc) []int {
    var code []int
    x, y := x0, y0

    for _, instruction := range instructions {
        for _, dir := range instruction {
            nx, ny := x, y
            switch dir {
                case 'U': ny--
                case 'D': ny++
                case 'R': nx++
                case 'L': nx--
                default: log.Fatalf("Invalid direction '%v'\n", dir)
            }
            if check(nx, ny) {
                x, y = nx, ny
            }
        }
        digit := getDigit(x, y)
        code = append(code, digit)
    }
    
    return code
}

func ParseInstructions(fileName string) []string {
    var instructions []string
    ReadFileLineByLine(fileName, func(line string) {
        if len(line) > 0 {
            instructions = append(instructions, line)
        }
    })
    return instructions
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


