package main

import "core:fmt"
import "core:os"
import "core:strings"
import "core:strconv"

Opcode :: enum{Rect, RotateRow, RotateColumn}

Instruction :: struct {
    Op : Opcode,
    A : int,
    B : int,
}

Screen :: struct {
    Width: int,
    Height: int,
    Pixels: []u8,
}

main :: proc() {

    when false {
        width :: 7
        height :: 3
        file_name :: "../example"
    } else {
        width :: 50
        height :: 6
        file_name :: "../input"
    }

    instructions, parse_ok := parse_instructions_file(file_name)
    if !parse_ok {
        fmt.println("failed to parse input file")
        return
    }

    screen := make_screen(width, height)

    for instr in instructions {
        execute_instruction(screen, instr)
    }

    print_screen(screen)

    fmt.printf("Lit pixels: %d\n", count_lit_pixels(screen))
    
}

count_lit_pixels :: proc(screen: ^Screen) -> (count: int) {
    for y in 0..<screen.Height {
        for x in 0..<screen.Width {
            if get_pixel(screen, x, y) != 0 {
                count += 1
            }
        }
    }
    return
}

get_pixel :: proc(screen: ^Screen, x: int, y: int) -> u8 {
    assert(x >= 0)
    assert(y >= 0)
    assert(x < screen.Width)
    assert(y < screen.Height)

    return screen.Pixels[y * screen.Width + x]
}

set_pixel :: proc(screen: ^Screen, x: int, y: int, value: u8) {
    assert(x >= 0)
    assert(y >= 0)
    assert(x < screen.Width)
    assert(y < screen.Height)

    screen.Pixels[y * screen.Width + x] = value
}

execute_instruction :: proc(screen: ^Screen, instruction: ^Instruction) {
    switch instruction.Op {
        case .Rect: {
            for y in 0..<instruction.B {
                for x in 0..<instruction.A {
                    set_pixel(screen, x, y, 1)
                }
            }
        }

        case .RotateRow: {
            row := instruction.A
            amount := instruction.B
            for repeat in 0..<amount {
                prev := get_pixel(screen, screen.Width-1, row)
                for i in 0..<screen.Width {
                    curr := get_pixel(screen, i, row)
                    set_pixel(screen, i, row, prev)
                    prev = curr
                }
                set_pixel(screen, 0, row, prev)
            }
        }

        case .RotateColumn: {
            column := instruction.A
            amount := instruction.B
            for repeat in 0..<amount {
                prev := get_pixel(screen, column, screen.Height-1)
                for i in 0..<screen.Height {
                    curr := get_pixel(screen, column, i)
                    set_pixel(screen, column, i, prev)
                    prev = curr
                }
                set_pixel(screen, column, 0, prev)
            }
        }
    }
}

make_screen :: proc(width, height: int) -> ^Screen {
    screen := new(Screen)
    screen.Width = width
    screen.Height = height
    screen.Pixels = make([]u8, width * height)
    return screen
}

print_screen :: proc(screen: ^Screen) {
    for y in 0..<screen.Height {
        for x in 0..<screen.Width {
            if get_pixel(screen, x, y) != 0 {
                fmt.printf("#")
            } else {
                fmt.printf(" ")
            }
        }
        fmt.printf("\n")
    }
}

parse_instructions_file :: proc(file_name: string) -> ([]^Instruction, bool) {

    data, ok := os.read_entire_file(file_name)
    if !ok {
        fmt.printf("Can't read file: '%s'\n", file_name)
        return nil, false
    }

    instructions := make([dynamic]^Instruction) 

    str := string(data)
    for line in strings.split_lines_iterator(&str) {
        instr, ok := parse_instruction(line)
        if !ok {
            fmt.printf("Can't parse line: '%s'\n", line)
            return nil, false
        }

        append(&instructions, instr)
    }

    return instructions[:], true // dynamic array to splice
}

parse_instruction :: proc(line: string) -> (instr: ^Instruction, ok: bool) {

    switch {

        case line[0:5] == "rect ":
        {
            args, split_err := strings.split(line[5:], "x")
            if split_err != nil do return

            x := strconv.parse_int(args[0]) or_return
            y := strconv.parse_int(args[1]) or_return

            i := new(Instruction)
            i.Op = Opcode.Rect
            i.A = x
            i.B = y
            return i, true
        }

        case line[0:16] == "rotate column x=":
        {
            args, split_err := strings.split(line[16:], " by ")
            if split_err != nil do return

            column := strconv.parse_int(args[0]) or_return
            dist := strconv.parse_int(args[1]) or_return

            i := new(Instruction)
            i.Op = Opcode.RotateColumn
            i.A = column
            i.B = dist
            return i, true
        }

        case line[0:13] == "rotate row y=":
        {
            args, split_err := strings.split(line[13:], " by ")
            if split_err != nil do return

            row := strconv.parse_int(args[0]) or_return
            dist := strconv.parse_int(args[1]) or_return

            i := new(Instruction)
            i.Op = Opcode.RotateRow
            i.A = row
            i.B = dist
            return i, true
        }

        case: return
    }
}

