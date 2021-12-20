let () =
    let rec main_loop () =
        let sysinfo: Metrics.metrics = {
            cpuinfo = Cpu.info();
            cpufreq = Cpufreq.info();
            meminfo = Mem.info();
            netinfo = Net.info();
        } in
        let () = Console_writer.write sysinfo in
        main_loop ()
    in  
    main_loop ()
        
