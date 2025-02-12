export interface IProblemDetails {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    instance?: string;
    extensions?: Record<string, any>;
}
