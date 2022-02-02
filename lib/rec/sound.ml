type sound_info = {
    mutable card: string;
    mutable mixer: string;
    mutable sink: string;
    mutable volume: int;
    mutable muted: bool;
}

let sound_info_i = {card="?";mixer="?";sink="?";volume=0;muted=false}
