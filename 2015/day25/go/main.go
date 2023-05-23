package main

import (
    "fmt"
)

func main() {
    fmt.Println("Hello!")

    const c0 uint64 = 20151125
    const k1 uint64 = 252533
    const k2 uint64 = 33554393

    var pos uint64 = 1
    var row uint64 = 1
    var col uint64 = 1
    code := c0

    for {
        if row == 2947 && col == 3029 {
            fmt.Printf("Found code: %d\npos=%d row=%d col=%d\n", code, pos, row, col)
            break
        }

        pos++

        if row == 1 {
            row = col + 1
            col = 1
        } else {
            row--
            col++
        }

        code = (code * k1) % k2
    }
}



