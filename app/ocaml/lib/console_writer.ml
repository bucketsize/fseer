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

let write_cput (p:Cputemp.cpu_temp) =
    p.temps
        |> (List.iter (fun x -> printf "cput:%3d " x))

let write_mem (p:Mem.mem_info) = 
    printf "mem:%3.0f " (p.usage *. 100.0)

let write_net (p:Net.net_info) = 
    p.intfs
    |> List.filter
        (fun (y:(string*Net.net_if)) -> 
            let _,x = y in
            (x.tx > 0L) && (x.name <> "lo"))
    |> List.iter
        (fun (y:(string*Net.net_if)) -> 
            let _,x = y in
            printf "%s: [%Ld %Ld (%.1f %.1f)] " x.name
                (Int64.div x.rx 1024L) 
                (Int64.div x.tx 1024L) x.dr x.dt)

let write_pwr (p:Power.power_info) = 
    if p.psu = "Psu"
    then printf "%s" p.psu
    else printf "%s %d %s" p.psu p.level p.status

let write (p:Metrics.metrics) = 
    write_cpu p.cpuinfo;
    write_cpuf p.cpufreq;
    write_cput p.cputemp;
    write_mem p.meminfo;
    write_net p.netinfo;
    write_pwr p.pwrinfo;
    print_newline ()

