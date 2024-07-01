import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AstronautDutiesComponent } from './astronaut-duties.component';
import { AstronautDutyService } from '../../services/astronaut-duty.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

describe('AstronautDutiesComponent', () => {
  let component: AstronautDutiesComponent;
  let fixture: ComponentFixture<AstronautDutiesComponent>;
  let dutyService: AstronautDutyService;
  let dutyServiceSpy: jasmine.Spy;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AstronautDutiesComponent],
      imports: [
        HttpClientTestingModule,
        FormsModule // Import FormsModule for ngModel
      ],
      providers: [AstronautDutyService]
    }).compileComponents();

    fixture = TestBed.createComponent(AstronautDutiesComponent);
    component = fixture.componentInstance;
    dutyService = TestBed.inject(AstronautDutyService);

    dutyServiceSpy = spyOn(dutyService, 'getDutiesByName').and.returnValue(of([]));
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show an error message if the search name is empty', () => {
    component.searchName = '';
    component.searchDuties();

    expect(component.errorMessage).toBe('Name is required');
    expect(component.loading).toBeFalse();
  });

  it('should fetch and display duties when searchDuties is called', () => {
    const mockDuties: any[] = [
      { id: 1, personId: 1, rank: 'Captain', dutyTitle: 'Commander', dutyStartDate: '2023-01-01', dutyEndDate: '2023-06-01' },
      { id: 2, personId: 1, rank: 'Major', dutyTitle: 'Pilot', dutyStartDate: '2023-06-02' }
    ];

    dutyServiceSpy.and.returnValue(of({ astronautDuties: mockDuties }));
    component.searchName = 'John Doe';
    component.searchDuties();

    expect(component.duties).toEqual(mockDuties);
    expect(component.errorMessage).toBe('');
    expect(component.loading).toBeFalse();
  });

  it('should show an error message if no duties are found', () => {
    dutyServiceSpy.and.returnValue(of({ astronautDuties: [] }));
    component.searchName = 'John Doe';
    component.searchDuties();

    expect(component.duties.length).toBe(0);
    expect(component.errorMessage).toBe('No duties found for John Doe');
    expect(component.loading).toBeFalse();
  });

  it('should handle errors from the service', () => {
    const errorResponse = { error: { message: 'Some error occurred' } };
    dutyServiceSpy.and.returnValue(throwError(() => errorResponse));
    component.searchName = 'John Doe';
    component.searchDuties();

    expect(component.duties.length).toBe(0);
    expect(component.errorMessage).toBe('Failed to retrieve duties: Some error occurred');
    expect(component.loading).toBeFalse();
  });
});
