import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Customer, CustomerStats, PagedCustomersResponse } from '../models/customer.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5234/api/customers';

  getCustomers(
    pageNumber: number,
    pageSize: number,
    name?: string,
    country?: string
  ): Observable<PagedCustomersResponse> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (name) {
      params = params.set('name', name);
    }

    if (country) {
      params = params.set('country', country);
    }

    return this.http.get<PagedCustomersResponse>(this.baseUrl, { params });
  }

  getCustomerById(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.baseUrl}/${id}`);
  }

  createCustomer(customer: any): Observable<Customer> {
    return this.http.post<Customer>(this.baseUrl, customer);
  }

  updateCustomer(id: number, customer: any): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, customer);
  }

  deleteCustomer(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  bulkDeactivate(ids: number[]): Observable<{ updatedCount: number }> {
    return this.http.post<{ updatedCount: number }>(`${this.baseUrl}/bulk-deactivate`, { ids });
  }

  getStats(): Observable<CustomerStats> {
    return this.http.get<CustomerStats>(`${this.baseUrl}/stats`);
  }
}