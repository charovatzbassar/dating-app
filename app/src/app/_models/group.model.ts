import { Connection } from "./connection.model";

export interface Group {
    name: string;
    connections: Connection[];
}

