import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AstronautDuty {
  id: number;
  personId: number;
  rank: string;
  dutyTitle: string;
  dutyStartDate: string;
  dutyEndDate?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AstronautDutyService {
  private apiUrl = 'http://localhost:5204/astronautduty';

  constructor(private http: HttpClient) { }

  getDutiesByName(name: string): Observable<AstronautDuty[]> {
    return this.http.get<AstronautDuty[]>(`${this.apiUrl}/${name}`);
  }
}
