type cpu_freq = {
    freqs: int32 list;
}

let seq_of_ints (s:int) (e:int) = 
    let rec seq_of_ints_p (s:int) (e:int) (acc: int list) =
        if s < e
            then seq_of_ints_p (s+1) e (s::acc)
            else acc
    in
    seq_of_ints_p s e []

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
            mfreq )
    in
    {freqs=freqs}

