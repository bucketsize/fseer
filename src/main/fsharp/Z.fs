open System.Threading

[<EntryPoint>]
let main argv =
    while true do
        Cpu.info(Writer.Cpu)
        CpuFreq.info(Writer.CpuFreq)
        CpuTemp.info(Writer.CpuTemp)
        Mem.info(Writer.Mem)
        Disks.info(Writer.Disks)
        Power.info(Writer.Power)
        Thread.Sleep(2000)
    0 
