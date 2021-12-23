open Futil

type cpu_freq = {
    freqs: int list;
}

let cpufreq_files =
    seq_of_ints 0 127
    |> List.map (Printf.sprintf "/sys/devices/system/cpu/cpu%d/cpufreq/scaling_cur_freq")
    |> List.filter (fun x -> Sys.file_exists x)

let info () =  
    let freqs =
        cpufreq_files
        |> List.map (fun f -> 
            let sfreq = (List.nth (Futil.read_file_lines f) 0) in
            let mfreq = Int32.div (Int32.of_string sfreq) 1000l in
            Int32.to_int mfreq )
    in
    {freqs = freqs}

