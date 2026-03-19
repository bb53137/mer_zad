import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, NgZone, OnInit, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CustomerStats } from '../../models/customer.model';
import { CustomerService } from '../../services/customer.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-stats-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './stats-dashboard.html',
  styleUrl: './stats-dashboard.scss'
})
export class StatsDashboardComponent implements OnInit {
  private customerService = inject(CustomerService);
  private cdr = inject(ChangeDetectorRef);
  private ngZone = inject(NgZone);

  stats?: CustomerStats;
  loading = false;

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.loading = true;

    this.customerService
      .getStats()
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
            this.stats = response;
            this.cdr.detectChanges();
          });
        },
        error: () => {
          this.ngZone.run(() => {
            alert('Greška kod učitavanja statistike.');
            this.cdr.detectChanges();
          });
        }
      });
  }
}