export interface PaginatedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    pageTotalCount: number;
    totalPages: number;
}