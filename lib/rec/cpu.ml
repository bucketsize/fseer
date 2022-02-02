type cpu_info = {
    mutable arch: string;
    mutable usage: float;
    mutable cs: int64;
    mutable cz: int64;
}
let cpu_info_i = {arch="?"; usage=0.0;cs=0L;cz=0L}

