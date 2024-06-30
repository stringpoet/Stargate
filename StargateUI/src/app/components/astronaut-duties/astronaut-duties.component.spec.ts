import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AstronautDutiesComponent } from './astronaut-duties.component';

describe('AstronautDutiesComponent', () => {
  let component: AstronautDutiesComponent;
  let fixture: ComponentFixture<AstronautDutiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AstronautDutiesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AstronautDutiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
