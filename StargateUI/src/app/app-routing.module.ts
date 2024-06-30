import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AstronautDutiesComponent } from './components/astronaut-duties/astronaut-duties.component';

const routes: Routes = [
  { path: '', component: AstronautDutiesComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
