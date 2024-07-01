import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AstronautDutyService, AstronautDuty } from './astronaut-duty.service';

describe('AstronautDutyService', () => {
  let service: AstronautDutyService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AstronautDutyService]
    });
    service = TestBed.inject(AstronautDutyService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensure that there are no outstanding HTTP requests
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch duties by name', () => {
    const dummyDuties: AstronautDuty[] = [
      { id: 1, personId: 1, rank: 'Captain', dutyTitle: 'Commander', dutyStartDate: '2023-01-01', dutyEndDate: '2023-06-01' },
      { id: 2, personId: 1, rank: 'Major', dutyTitle: 'Pilot', dutyStartDate: '2023-06-02' }
    ];

    service.getDutiesByName('John Doe').subscribe(duties => {
      expect(duties.length).toBe(2);
      expect(duties).toEqual(dummyDuties);
    });

    const req = httpMock.expectOne('http://localhost:5204/astronautduty/John Doe');
    expect(req.request.method).toBe('GET');
    req.flush(dummyDuties);
  });

  it('should handle empty results', () => {
    service.getDutiesByName('Jane Doe').subscribe(duties => {
      expect(duties.length).toBe(0);
    });

    const req = httpMock.expectOne('http://localhost:5204/astronautduty/Jane Doe');
    expect(req.request.method).toBe('GET');
    req.flush([]); // Return an empty array
  });

  it('should handle error response', () => {
    service.getDutiesByName('John Doe').subscribe(
      duties => fail('should have failed with the 404 error'),
      (error) => {
        expect(error.status).toBe(404);
      }
    );

    const req = httpMock.expectOne('http://localhost:5204/astronautduty/John Doe');
    expect(req.request.method).toBe('GET');
    req.flush('404 error', { status: 404, statusText: 'Not Found' });
  });
});
