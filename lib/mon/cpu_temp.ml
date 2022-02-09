open Fseer.Futil
open Fseerrec.Cpu_temp
open Printf

let cputemp_files_ryzen = 
    let i = seq_of_ints 0 15 and
        j = seq_of_ints 0 3 in
    let k = i |> List.map (fun x -> 
            j |> List.map (fun y -> (x, y))) in
    let l = k |> List.flatten in
    l 
        |> List.map (fun (i, j) -> 
                ((i, j), Printf.sprintf "/sys/class/hwmon/hwmon%d/temp%d_label" i j))
        |> List.filter (fun (_, f) -> 
                Sys.file_exists f)
        |> List.map (fun ((i, j), _) -> 
                (Printf.sprintf "/sys/class/hwmon/hwmon%d/temp%d_input" i j))

let cputemp_files_pi4 = 
    ["/sys/class/thermal/thermal_zone0/temp"]
    |> List.filter (fun f -> Sys.file_exists f)

let cputemp_files =
    List.append 
        cputemp_files_ryzen
        cputemp_files_pi4

let info (m: Fseerrec.Metrics.metrics) zfn = 
    let temps = 
        cputemp_files
        |> List.map (fun x -> 
                printf "temp file: %s\n" x;
                let ls = read_file_lines x in
                List.nth ls 0)
        |> List.map (fun x -> 
                printf "temp: %s\n" x;
                Int32.to_int (Int32.div (Int32.of_string x) 1000l))
    in
    let tmax = (* TODO: get correct tmin *)
        if List.length temps > 0
            then List.nth temps 0
            else 0
    in 
    m.cpu_temp <- {temps = temps; temp_max = tmax}


