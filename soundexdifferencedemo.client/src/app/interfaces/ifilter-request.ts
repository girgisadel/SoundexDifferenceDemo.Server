import { IPaginationRequest } from "./ipagination-request";

export interface IFilterRequest extends IPaginationRequest {
    searchTerm?: string;
    createdAtFrom?: Date;
    createdAtTo?: Date;
    orderBy: string;
    isDescending: boolean;
}
