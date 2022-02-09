open Fseer.Futil
open Fseerrec.Cpu_freq

let cpufreq_files =
    seq_of_ints 0 127
    |> List.map (Printf.sprintf "/sys/devices/system/cpu/cpu%d/cpufreq/scaling_cur_freq")
    |> List.filter (fun x -> Sys.file_exists x)

let info (m: Fseerrec.Metrics.metrics) zfn =  
    let freqs =
        cpufreq_files
        |> List.map (fun f -> 
            let sfreq = (List.nth (read_file_lines f) 0) in
            let mfreq = Int32.div (Int32.of_string sfreq) 1000l in
            Int32.to_int mfreq )
    in
    let fsum = 
        freqs 
        |> (List.fold_left (fun s x -> s + x) 0)
    in
    let favg = 
        if List.length freqs > 0
            then fsum / List.length freqs 
            else 0
    in    
    m.cpu_freq <- {freqs = freqs;freq_tot=fsum;freq_avg=favg}

