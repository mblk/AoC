package main

import (
    "fmt"
    "os"
    "log"
    "bufio"
    "strconv"
    "sort"
    "math"
    "time"
)

type WeightData struct {
    numBuckets uint32
    weights []uint32
    totalWeight uint32
    weightPerBucket uint32
}

type Bucket struct {
    weight uint32
    count uint32
    entanglement uint64
}

type Solution struct {
    minItemsInFirstBucket uint32
    minQuantumEntanglement uint64
}

func main() {
    fmt.Print("Hello!\n")

    if len(os.Args) != 3 {
        log.Fatal("Missing arguments. Usage: ./tool [inputFile] [numBuckets]\n")
    }

    var numBuckets uint32
    if value, err := strconv.Atoi(os.Args[2]); err != nil {
        log.Fatal("Can't parse number of buckets")
    } else {
        numBuckets = uint32(value)
    }
    if numBuckets < 2 {
        log.Fatal("Illegal bucket count")
    }

    weightData := parseWeightData(os.Args[1], numBuckets)

    //fmt.Printf("numBuckets: %d\n", numBuckets)
    //fmt.Printf("weightData: %v\n", weightData)

    solution := Solution{
        minItemsInFirstBucket: math.MaxUint32,
        minQuantumEntanglement: math.MaxUint64,
    }

    buckets := make([]Bucket, numBuckets)
    for idx := range buckets {
        buckets[idx].entanglement = 1
    }

    startTime := time.Now()
    findSolutions(weightData, &solution, buckets, 0)
    duration := time.Since(startTime)

    if solution.minQuantumEntanglement == math.MaxUint32 {
        fmt.Println("No solution found!")
    } else {
        fmt.Printf("Solution: %d items, %d quantum entanglement, duration %v\n", solution.minItemsInFirstBucket, solution.minQuantumEntanglement, duration)
    }
}

func findSolutions(data *WeightData, solution *Solution, buckets []Bucket, currentIndex int) {
    // All weights placed?
    if currentIndex == len(data.weights) {
        // Better solution?
        if buckets[0].count < solution.minItemsInFirstBucket ||
           buckets[0].count == solution.minItemsInFirstBucket && buckets[0].entanglement < solution.minQuantumEntanglement {
            // Update solution.
            solution.minItemsInFirstBucket = buckets[0].count
            solution.minQuantumEntanglement = buckets[0].entanglement
            //fmt.Printf("New best #1: items=%d entanglement=%d\n", solution.minItemsInFirstBucket, solution.minQuantumEntanglement)
            //fmt.Printf("%v\n", buckets)
        }
        return
    }

    currentWeight := data.weights[currentIndex]

    for pick:=uint32(0); pick<data.numBuckets; pick++ {

        // overflow?
        if buckets[pick].weight + currentWeight > data.weightPerBucket {
            continue;
        }

        // placing weight in first container?
        if pick == 0 {
            mightBeBetterSolution := false
            // lower number of items?
            if buckets[0].count + 1 < uint32(solution.minItemsInFirstBucket) {
                mightBeBetterSolution = true
            }
            // same number of items but lower quantum entanglement?
            if buckets[0].count + 1 == uint32(solution.minItemsInFirstBucket) &&
               buckets[0].entanglement * uint64(currentWeight) < solution.minQuantumEntanglement {
                mightBeBetterSolution = true
            }
            if !mightBeBetterSolution {
                continue
            }
        }

        bucketBackup := buckets[pick]
        
        buckets[pick].weight       += currentWeight
        buckets[pick].count        += 1
        buckets[pick].entanglement *= uint64(currentWeight)

        findSolutions(data, solution, buckets, currentIndex + 1)
          
        buckets[pick] = bucketBackup
    }
}

func parseWeightData(fileName string, numBuckets uint32) *WeightData {
    var weights []uint32

    readFileLineByLine(fileName, func(line string) {
        if value, err := strconv.ParseInt(line, 10, 32); err == nil {
            weights = append(weights, uint32(value))
        }
    })

    // Sort descending
    sort.Slice(weights, func(a, b int) bool { return weights[a] > weights[b] })

    var totalWeight uint32
    for _, val := range weights {
        totalWeight += val
    }

    weightPerBucket := totalWeight / numBuckets

    return &WeightData{
        numBuckets: numBuckets,
        weights: weights,
        totalWeight: totalWeight,
        weightPerBucket: weightPerBucket,
    }
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



