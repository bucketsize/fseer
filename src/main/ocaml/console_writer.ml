open Printf

let write (p:Metrics.metrics) = 
    printf "{";
    p.cpuinfo.usage |> (fun x -> printf "cpu:%3.0f " (x*.100.0));
    printf ",cpuFreq: ";
    p.cpufreq.freqs |> List.iter (fun f -> printf "%4d " (Int32.to_int f));
    printf ",cputemp: ";
    (* p.cputemp.temps |> List.iter (fun f -> printf "%3d " f); *)
    p.meminfo.usage |> (fun x -> printf ",mem: %3.0f " (x*.100.0));
    (* p.dskinfo.disks |> List.iter (fun f -> *) 
    (*     printf ",part %s: %3d, %3d " f.dev f.riops f.wiops); *)
    (* p.pwrinfo |> (fun x -> printf "%s: %s, %2d " x.psu x.status x.level); *)
    p.netinfo.intfs |> List.iter (fun (f:Net.net_if) -> 
        printf ",If %s: %s / %s " f.intf (Int64.to_string f.rx) (Int64.to_string f.tx));
    printf "}";
