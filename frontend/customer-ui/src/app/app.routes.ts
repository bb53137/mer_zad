import { Routes } from '@angular/router';
import { CustomerListComponent } from './pages/customer-list/customer-list';
import { CustomerEditComponent } from './pages/customer-edit/customer-edit';
import { StatsDashboardComponent } from './pages/stats-dashboard/stats-dashboard';

export const routes: Routes = [
  { path: '', redirectTo: 'customers', pathMatch: 'full' },
  { path: 'customers', component: CustomerListComponent },
  { path: 'customers/:id', component: CustomerEditComponent },
  { path: 'stats', component: StatsDashboardComponent }
];