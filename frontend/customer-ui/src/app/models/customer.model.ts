export interface Customer {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string | null;
  city: string;
  country: string;
  isActive: boolean;
  createdAt: string;
  lastModifiedAt?: string | null;
}

export interface PagedCustomersResponse {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  items: Customer[];
}

export interface CustomerStats {
  totalCount: number;
  activeCount: number;
  inactiveCount: number;
  topCities: TopCity[];
}

export interface TopCity {
  city: string;
  count: number;
}