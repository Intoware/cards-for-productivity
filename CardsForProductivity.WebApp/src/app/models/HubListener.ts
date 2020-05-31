export interface HubListener {
    name: string;
    newMethod: (...args: any[]) => void;
}
