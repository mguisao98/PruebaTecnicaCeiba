import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventosList } from './eventos-list';

describe('EventosList', () => {
  let component: EventosList;
  let fixture: ComponentFixture<EventosList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EventosList],
    }).compileComponents();

    fixture = TestBed.createComponent(EventosList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
