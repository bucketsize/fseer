type mem_info = {
    mutable usage: float;
    mutable mt: int64;
    mutable mf: int64;
    mutable st: int64;
    mutable sf: int64;
}

let mem_info_i = {usage=0.0;mt=0L;mf=0L;st=0L;sf=0L}
