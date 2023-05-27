package main

import (
    "fmt"
    "os"
    "bufio"
    "log"
)

func main() {
    if len(os.Args) != 2 {
        log.Fatal("Missing argument")
    }

    input := ReadInputFile(os.Args[1])

    tlsCount := 0
    sslCount := 0
    for _, line := range input {
        if AddressSupportsTls(line) {
            //fmt.Printf("TLS: %v\n", line)
            tlsCount++
        }
        if AddressSupportsSsl(line) {
            //fmt.Printf("SSL: %v\n", line)
            sslCount++
        }
    }

    fmt.Printf("Total addresses: %d\n", len(input))
    fmt.Printf("TLS addresses: %d\n", tlsCount)
    fmt.Printf("SSL addresses: %d\n", sslCount)
}

func AddressSupportsSsl(address string) bool {
    inBrackets := false

    var abas [][]rune // Area broadcast accessor
    var babs [][]rune // Byte allocation block

    for i:=0; i<len(address)-2; i++ {
        c := address[i]

        switch c {
        case '[': inBrackets = true
        case ']': inBrackets = false
        default:
            sub := []rune(address[i:i+3])
            if IsAba(sub) {
                if inBrackets {
                    babs = append(babs, sub)
                } else {
                    abas = append(abas, sub)
                }
            }
        }
    }

    // Check if there are any matching ABAs & BABs
    for _, aba := range abas {
        for _, bab := range babs {
            if aba[0] == bab[1] && bab[0] == aba[1] {
                return true
            }
        }
    }

    return false
}

func AddressSupportsTls(address string) bool {

    inBrackets := false

    hasAbbaInsideBrackets := false
    hasAbbaOutsideBrackets := false

    for i:=0; i<len(address)-3; i++ {
        c := address[i]

        switch c {
        case '[': inBrackets = true
        case ']': inBrackets = false
        default:
            sub := []rune(address[i:i+4])
            if IsAbba(sub) {
                if inBrackets {
                    hasAbbaInsideBrackets = true
                } else {
                    hasAbbaOutsideBrackets = true
                }
            }
        }
    }

    return hasAbbaOutsideBrackets && !hasAbbaInsideBrackets
}

func IsAba(seq []rune) bool {
    if len(seq) != 3 {
        log.Fatal("Invalid sequence passed to IsAba", seq)
    }
    return seq[0] == seq[2] && seq[0] != seq[1]
}

func IsAbba(seq []rune) bool {
    if len(seq) != 4 {
        log.Fatal("Invalid sequence passed to IsAbba", seq)
    }
    return seq[0] == seq[3] &&
        seq[1] == seq[2] &&
        seq[0] != seq[1]
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


