open Futil

type cpu_temp = {
    temps: int list;
}

let cputemp_files_ryzen = 
    let i = seq_of_ints 0 15 and
        j = seq_of_ints 0 3 in
    let k = i |> List.map (fun x -> 
            j |> List.map (fun y -> (x, y))) in
    let l = k |> List.flatten in
    l 
        |> List.map (fun (i1, j1) -> 
                Printf.sprintf "/sys/class/hwmon/hwmon%d/temp%d_label" i1 j1
            )
        |> List.filter (fun f -> Sys.file_exists f)

let cputemp_files_pi4 = 
    ["/sys/class/thermal/thermal_zone0/temp"]

let cputemp_files =
    List.append 
        cputemp_files_ryzen
        cputemp_files_pi4

let info () = 
    let temps = 
        cputemp_files
        |> List.map (fun x -> 
                let ls = read_file_lines x in
                List.nth ls 0)
        |> List.map (fun x -> Int32.to_int (Int32.div (Int32.of_string x) 1000l))
    in
    {temps = temps}


