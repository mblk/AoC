package main

import (
    "fmt"
    "crypto/md5"
    "io"
)

func main() {
    fmt.Println("Hello!")

    const doorId = "cxdnnyjw"
    //const doorId = "abc"

    password1 := GeneratePassword1(doorId)
    password2 := GeneratePassword2(doorId)

    fmt.Printf("Passwort1: '%v'\n", password1)
    fmt.Printf("Passwort2: '%v'\n", password2)
}

func GeneratePassword1(doorId string) string {

    characters := make([]rune, 8)
    for idx := range characters {
        characters[idx] = '_'
    }

    increasingIndex := uint64(0)
    for charNum:=0; charNum<8; charNum++ {
        fmt.Printf("\rGenerating %d/8: %v", charNum, string(characters))
        for {
            hashInput := fmt.Sprintf("%v%d", doorId, increasingIndex)
            increasingIndex++

            h := md5.New()
            io.WriteString(h, hashInput)
            hashOutput := fmt.Sprintf("%x", h.Sum(nil))

            if hashOutput[0:5] == "00000" {
                characters[charNum] = rune(hashOutput[5])
                break
            }
        }
    }

    fmt.Printf("\rGenerated: %v          \n", string(characters))
    return string(characters)
}

func GeneratePassword2(doorId string) string {

    characters := make([]rune, 8)
    for idx := range characters {
        characters[idx] = '_'
    }

    increasingIndex := uint64(0)
    for charNum:=0; charNum<8; charNum++ {
        fmt.Printf("\rGenerating %d/8: %v", charNum, string(characters))
        for {
            hashInput := fmt.Sprintf("%v%d", doorId, increasingIndex)
            increasingIndex++

            h := md5.New()
            io.WriteString(h, hashInput)
            hashOutput := fmt.Sprintf("%x", h.Sum(nil))

            if hashOutput[0:5] == "00000" {
                position := hashOutput[5] - '0'
                if position < 8 && characters[position] == '_' {
                    characters[position] = rune(hashOutput[6])
                    break
                }
            }
        }
    }

    fmt.Printf("\rGenerated: %v          \n", string(characters))
    return string(characters)
}

