package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strings"
    "strconv"
)

type Program struct {
    instructions []Instruction
}

type Instruction struct {
    line string
    op Opcode
    reg uint8
    arg int64
}

type Opcode uint8
const (
    Illegal Opcode = iota
    Half
    Triple
    Increment
    Jump
    JumpIfEven
    JumpIfOne
    Print
)

type State struct {
    pc uint64
    regs [2]uint64
}

func (i Instruction) String() string {
    return fmt.Sprintf("'%v' op=%d reg=%d arg=%d", i.line, i.op, i.reg, i.arg)
}

func (s State) String() string {
    return fmt.Sprintf("pc=%d a=%d b=%d", s.pc, s.regs[0], s.regs[1])
}

func makeState(a uint64, b uint64) State {
    return State{
        pc: 0,
        regs: [2]uint64{a,b},
    }
}

func main() {
    if len(os.Args) < 2 {
        log.Fatal("No program specified")
    }

    program := parseProgramFile(os.Args[1])
   
    part1 := program.run(makeState(0, 0))
    part2 := program.run(makeState(1, 0))

    fmt.Printf("Part1: %v\n", part1)
    fmt.Printf("Part2: %v\n", part2)
}

func (program *Program) run(startingState State) State {
    state := startingState

    for int(state.pc) < len(program.instructions) {
        instr := program.instructions[state.pc]
        state = instr.execute(state)
    }

    return state
}

func (i *Instruction) execute(oldState State) State {
    newState := oldState
    newState.pc = oldState.pc + 1

    switch i.op {
        case Half:          newState.regs[i.reg] /= 2
        case Triple:        newState.regs[i.reg] *= 3
        case Increment:     newState.regs[i.reg] += 1
        case Jump:          newState.pc = oldState.pc + uint64(i.arg)
        case JumpIfEven:    if oldState.regs[i.reg] % 2 == 0 {
                                newState.pc = oldState.pc + uint64(i.arg)
                            }
        case JumpIfOne:     if oldState.regs[i.reg] == 1 {
                                newState.pc = oldState.pc + uint64(i.arg)
                            }
        case Print:         fmt.Printf("Print: pc=%d a=%d b=%d\n", oldState.pc, oldState.regs[0], oldState.regs[1])
        default:            log.Fatal("Invalid instruction", i)
    }

    return newState
}

func parseProgramFile(fileName string) *Program {
    var instructions []Instruction

    readFileLineByLine(fileName, func(line string) {
        instruction := parseInstructionLine(line)
        instructions = append(instructions, instruction)
    })

    fmt.Printf("Found %d instructions in '%v'\n", len(instructions), fileName)
    return &Program{instructions}
}

func readFileLineByLine(fileName string, fn func(string)) {
    file, err := os.Open(fileName)
    if err != nil {
        log.Fatal(err)
    }
    defer file.Close()

    scanner := bufio.NewScanner(file)
    for scanner.Scan() {
        if err := scanner.Err(); err != nil {
            log.Fatal(err)
        }
        fn(scanner.Text())
    }
}

func parseInstructionLine(line string) Instruction {
    instr := Instruction{
        op: parseOpcode(line[0:3]),
    }

    arguments := strings.FieldsFunc(line[4:], func(r rune) bool {
        return r == ' ' || r == ','
    })

    for _, argument := range(arguments) {
        switch argument {
            case "a":   instr.reg = 0
            case "b":   instr.reg = 1
            default:    if num, err := strconv.ParseInt(argument, 10, 64); err == nil {
                            instr.arg = int64(num)
                        }
        }
    }

    return instr
}

func parseOpcode(s string) Opcode {
    switch s {
        case "hlf": return Half
        case "tpl": return Triple
        case "inc": return Increment
        case "jmp": return Jump
        case "jie": return JumpIfEven
        case "jio": return JumpIfOne
        case "prt": return Print
        default:
            log.Fatalf("Invalid instruction '%v'", s)
            return Illegal
    }
}

