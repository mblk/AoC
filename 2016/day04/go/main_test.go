package main

import (
    "testing"
)


func TestRoomChecksum(t *testing.T) {
    testCases := []struct{
        Name string
        Checksum string
    }{
        { "aaaaa-bbb-z-y-x", "abxyz" },
        { "a-b-c-d-e-f-g-h", "abcde" },
        { "not-a-real-room", "oarel" },

        { "abcde", "abcde" },
        { "abbcccddddeeeee", "edcba" },

        { "abc", "abc" },
        { "", "" },
    }

    for _, testCase := range testCases {
        calculatedChecksum := CalculateRoomChecksum(testCase.Name)
        if calculatedChecksum != testCase.Checksum {
            t.Fatalf("Incorrect checksum for room '%v' -> '%v' should be '%v'\n",
                testCase.Name, calculatedChecksum, testCase.Checksum)
        }
    }
}
