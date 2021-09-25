open System.Threading

[<EntryPoint>]
let main argv =
    while true do
        Cpu.usage()
        Mem.usage()
        CpuTemp.thermalinfo()
        CpuFreq.cpuFreqs()
        Power.status()
        Disks.disksio()
        Thread.Sleep(2000)
    0 
