package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
	"strings"
)

func main() {
    fmt.Println("Hello!")
    if len(os.Args) != 2 {
        log.Fatal("Invalid args")
    }
    fileName := os.Args[1]

    triangles1 := ParseTrianglesHorizontally(fileName)
    triangles2 := ParseTrianglesVertically(fileName)

    numLegal1 := CountLegalTriangles(triangles1)
    numLegal2 := CountLegalTriangles(triangles2)

    fmt.Printf("Part1\nTotal: %d\nLegal: %d\n", len(triangles1), numLegal1)
    fmt.Printf("Part2\nTotal: %d\nLegal: %d\n", len(triangles2), numLegal2)
}

func CountLegalTriangles(triangles [][3]int) int {
    numLegal := 0
    for _, tri := range triangles {
        if TriangleIsValid(tri) {
            numLegal++
        }
    }
    return numLegal
}

func TriangleIsValid(triangle [3]int) bool {
    perms := [] struct {
        a, b, c int
    }{
        // c must hit all indices
        {0,1,2},
        {0,2,1},
        {1,2,0},
    }

    for _, perm := range perms {
        if triangle[perm.a] + triangle[perm.b] <= triangle[perm.c] {
            return false
        }
    }

    return true
}

func ParseTrianglesHorizontally(fileName string) [][3]int {
    var triangles [][3]int
    
    ReadFileLineByLine(fileName, func(line string) {
        tri := ParseTriangleLine(line)
        triangles = append(triangles, tri)
    })
    
    return triangles
}

func ParseTrianglesVertically(fileName string) [][3]int {
    
    data := ParseTrianglesHorizontally(fileName)
    if len(data) % 3 != 0 {
        log.Fatal("Number of rows is not multiple of 3")
    }

    var triangles [][3]int

    for row:=0; row<len(data); row+=3 {
        for col:=0; col<3; col++ {
            triangles = append(triangles, [3]int{
                data[row+0][col],
                data[row+1][col],
                data[row+2][col],
            })
        }
    }
    
    return triangles
}

func ParseTriangleLine(line string) [3]int {

    fields := strings.Fields(line)
    if len(fields) != 3 {
        log.Fatalf("Invalid line: '%v'\n", line)
    }

    var triangle [3]int
    i := 0

    for _, field := range fields {

        number, err := strconv.ParseInt(field, 10, 0)
        if err != nil {
            log.Fatalf("Failed to parse '%v'\n", field)
        }

        triangle[i] = int(number)
        i++
    }

    return triangle
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


