open Printf
open Fseerrec

let write_cpu (p:Cpu.cpu_info) = 
    printf "cpu:%3.0f " (p.usage *. 100.0)

let write_cpuf (p:Cpu_freq.cpu_freq) =
    printf "cpuf:%4d " p.freq_avg

let write_cput (p:Cpu_temp.cpu_temp) =
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
    else printf "%s %d %s " p.psu p.level p.status

let write_snd (p:Sound.sound_info) = 
    printf "snd:[ %s %s %s %d %s] " p.card
        p.mixer
        p.sink
        p.volume
        (if p.muted then "muted" else "")

let write (p:Metrics.metrics) = 
    write_cpu p.cpu_info;
    write_cpuf p.cpu_freq;
    write_cput p.cpu_temp;
    write_mem p.mem_info;
    write_net p.net_info;
    write_snd p.snd_info;
    write_pwr p.pwr_info;
    print_newline ()

