open Printf

let write_cpu (p:Cpu.cpu_info) = 
    printf "cpu:%3.0f " (p.usage *. 100.0)

let write_cpuf (p:Cpufreq.cpu_freq) =
    let fsum = 
        p.freqs 
        |> (List.fold_left (fun s x -> s + x) 0)
    in
    let favg = fsum / List.length p.freqs in
    printf "cpuf:%4d " favg

let write_mem (p:Mem.mem_info) = 
    printf "mem:%3.0f " p.usage

let write_net (p:Net.net_info) = 
    let () =printf "net" in
    p.intfs
    |> List.filter
        (fun (x:Net.net_if) -> (x.tx > 0L))
    |> List.iter
        (fun (x:Net.net_if) -> printf " %8s: %Ld %Ld " x.intf x.rx x.tx)

let write (p:Metrics.metrics) = 
    write_cpu p.cpuinfo;
    write_cpuf p.cpufreq;
    write_mem p.meminfo;
    write_net p.netinfo;
    print_newline ()

