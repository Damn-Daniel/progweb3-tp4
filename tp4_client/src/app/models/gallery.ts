import { Picture } from "./picture";


export class Gallery {
    constructor(
        public id: number,
        public name: string,
        public picCouvertureID: number | null,
        public pictures: Picture[],
    ) { }
}