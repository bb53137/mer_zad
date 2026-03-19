import {CommonModule} from '@angular/common';
import {ChangeDetectorRef, Component, NgZone, OnInit, inject} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {Router, RouterModule} from '@angular/router';
import {Customer} from '../../models/customer.model';
import {CustomerService} from '../../services/customer.service';
import {Subject, debounceTime, distinctUntilChanged, finalize} from 'rxjs';


@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './customer-list.html',
  styleUrls: ['./customer-list.scss']
})
export class CustomerListComponent implements OnInit {
  private customerService = inject(CustomerService);
  private router = inject(Router);
  private searchSubject = new Subject<string>();
  private cdr = inject(ChangeDetectorRef);
  private ngZone = inject(NgZone);

  customers: Customer[] = [];
  selectedIds: number[] = [];
  loading = false;

  pageNumber = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;

  searchTerm = '';
  selectedCountry = '';

  countries: string[] = ['','Croatia','Njemačka','Austrija','Španjolska','Francuska','Srbija','Crna Gora','Italija','Mađaraska','Poljska','Nizozemska','Švicarska','Albanija','Bosna i Hercegovina'];

  ngOnInit(): void {
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(value => {
        const trimmed = value.trim();

        if (trimmed.length === 0 || trimmed.length >= 2) {
          this.pageNumber = 1;
          this.loadCustomers();
        }
      });

    this.loadCustomers();
  }

  loadCustomers(): void {
    this.loading = true;

    this.customerService
      .getCustomers(
        this.pageNumber,
        this.pageSize,
        this.searchTerm || undefined,
        this.selectedCountry || undefined
      )
      .pipe(
        finalize(() => {
          this.ngZone.run(() => {
            this.loading = false;
            this.cdr.detectChanges();
          });
        })
      )
      .subscribe({
        next: (response) => {

          this.ngZone.run(() => {
            this.customers = [...response.items];
            this.totalCount = response.totalCount;
            this.totalPages = response.totalPages;
            this.cdr.detectChanges();
          });
        },
        error: (error) => {
          this.ngZone.run(() => {
            console.log('API error:', error);
            alert('Greška kod učitavanja korisnika.');
            this.cdr.detectChanges();
          });
        }
      });
  }

  onSearch(): void {
    this.searchSubject.next(this.searchTerm);
  }

  onCountryChange(): void {
    this.pageNumber = 1;
    this.loadCustomers();
  }

  previousPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.loadCustomers();
    }
  }

  nextPage(): void {
    if (this.pageNumber < this.totalPages) {
      this.pageNumber++;
      this.loadCustomers();
    }
  }

  toggleSelection(customerId: number, checked: boolean): void {
    if (checked) {
      if (!this.selectedIds.includes(customerId)) {
        this.selectedIds.push(customerId);
      }
    } else {
      this.selectedIds = this.selectedIds.filter(id => id !== customerId);
    }
  }

  deactivateSelected(): void {
    if (this.selectedIds.length === 0) {
      alert('Odaberi barem jednog korisnika.');
      return;
    }

    this.customerService.bulkDeactivate(this.selectedIds).subscribe({
      next: (response) => {
        alert(`Deaktivirano: ${response.updatedCount}`);
        this.selectedIds = [];
        this.loadCustomers();
      },
      error: () => {
        alert('Greška kod bulk deactivate.');
      }
    });
  }

  editCustomer(id: number): void {
    this.router.navigate(['/customers', id]);
  }

  getFullName(customer: Customer): string {
    return `${customer.firstName} ${customer.lastName}`;
  }
}