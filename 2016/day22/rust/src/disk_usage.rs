use std::str::FromStr;

#[derive(Debug)]
pub struct DiskUsage {
    pub path: String,
    pub px: usize,
    pub py: usize,
    pub size: usize,
    pub used: usize,
    pub avail: usize,
    pub usage: usize,
}

#[derive(Debug)]
pub struct DiskUsageParseError;

impl FromStr for DiskUsage {
    type Err = DiskUsageParseError;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let parts = s
            .split_whitespace()
            .map(|s| s.trim_end_matches(&['T', '%']))
            .collect::<Vec<&str>>();

        let position = parts[0]
            .split('-')
            .skip(1)
            .map(|s| s.trim_start_matches(&['x', 'y']))
            .map(|s| s.parse::<usize>().unwrap())
            .collect::<Vec<usize>>();

        return Ok(DiskUsage {
            path: parts[0].to_string(),
            px: position[0],
            py: position[1],
            size: parts[1].parse().unwrap(),
            used: parts[2].parse().unwrap(),
            avail: parts[3].parse().unwrap(),
            usage: parts[4].parse().unwrap(),
        });
    }
}

impl DiskUsage {
    pub fn parse_file(filename: &str) -> Vec<DiskUsage> {
        std::fs::read_to_string(filename)
            .expect("can't read file")
            .lines()
            .filter(|s| s.starts_with("/dev"))
            .map(|s| s.parse().unwrap())
            .collect()
    }
}
