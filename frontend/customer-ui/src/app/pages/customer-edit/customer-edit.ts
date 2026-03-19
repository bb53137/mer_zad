import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, NgZone, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CustomerService } from '../../services/customer.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-customer-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './customer-edit.html',
  styleUrl: './customer-edit.scss'
})
export class CustomerEditComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private customerService = inject(CustomerService);
  private cdr = inject(ChangeDetectorRef);
  private ngZone = inject(NgZone);

  customerId = 0;
  loading = false;

  form = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    phone: [''],
    city: ['', [Validators.required]],
    country: ['', [Validators.required]],
    isActive: [true]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.customerId = Number(id);

    if (this.customerId) {
      this.loadCustomer();
    }
  }

  loadCustomer(): void {
    this.loading = true;

    this.customerService
      .getCustomerById(this.customerId)
      .pipe(
        finalize(() => {
          this.ngZone.run(() => {
            this.loading = false;
            this.cdr.detectChanges();
          });
       })
      )
      .subscribe({
        next: (customer) => {
          this.ngZone.run(() => {
            this.form.patchValue({
              firstName: customer.firstName,
              lastName: customer.lastName,
              email: customer.email,
              phone: customer.phone ?? '',
              city: customer.city,
              country: customer.country,
              isActive: customer.isActive
            });
            this.cdr.detectChanges();
          });
        },

        error: () => {
          this.ngZone.run(() => {
            alert('Greška kod učitavanja korisnika.');
            this.cdr.detectChanges();
          });
        }
      });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.customerService.updateCustomer(this.customerId, this.form.value).subscribe({
      next: () => {
        alert('Korisnik uspješno spremljen.');
        this.router.navigate(['/customers']);
      },
      error: () => {
        alert('Greška kod spremanja.');
      }
    });
  }
}