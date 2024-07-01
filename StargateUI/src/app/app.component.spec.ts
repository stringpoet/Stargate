import { TestBed, ComponentFixture } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { AstronautDutiesComponent } from './components/astronaut-duties/astronaut-duties.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { AstronautDutyService } from './services/astronaut-duty.service';

describe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;
  let component: AppComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent, AstronautDutiesComponent],
      imports: [HttpClientTestingModule, FormsModule],
      providers: [AstronautDutyService]
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it(`should have as title 'Astronaut Career Tracking System (ACTS)'`, () => {
    expect(component.title).toEqual('Astronaut Career Tracking System (ACTS)');
  });

  it('should render title in a h1 tag', () => {
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Astronaut Career Tracking System (ACTS)');
  });
});
