import { Component } from '@angular/core';
import { AstronautDutyService, AstronautDuty } from '../../services/astronaut-duty.service';

@Component({
  selector: 'app-astronaut-duties',
  templateUrl: './astronaut-duties.component.html',
  styleUrls: ['./astronaut-duties.component.css']
})
export class AstronautDutiesComponent {
  duties: AstronautDuty[] = [];
  loading = false;
  typing = false;
  errorMessage = '';
  searchName = '';
  displayedColumns: string[] = ['dutyTitle', 'rank', 'dutyStartDate', 'dutyEndDate'];

  constructor(private dutyService: AstronautDutyService) { }

  searchDuties() {
    this.typing = false;
    this.loading = true;
    this.errorMessage = '';
    this.duties = [];

    if (!this.searchName.trim()) {
      this.errorMessage = 'Name is required';
      this.loading = false;
      return;
    }

    this.dutyService.getDutiesByName(this.searchName).subscribe({
      next: (response: any) => {
        this.duties = Array.isArray(response.astronautDuties) ? response.astronautDuties : [];
        if (this.duties.length === 0) {
          this.errorMessage = 'No duties found for '.concat(this.searchName);
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error retrieving duties:', error);
        this.errorMessage = 'Failed to retrieve duties: ' + error.error.message;
        this.loading = false;
      }
    });
  }
}
