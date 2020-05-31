import { StoryModel } from './../../models/StoryModel';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-sidebar-item',
  templateUrl: './sidebar-item.component.html',
  styleUrls: ['./sidebar-item.component.scss']
})
export class SidebarItemComponent implements OnInit {

  @Input() story: StoryModel;
  @Input() cloneDisabled: boolean;
  @Input() deleteDisabled: boolean;
  @Input() isDuplicate: boolean;

  @Output() delete = new EventEmitter<StoryModel>();
  @Output() clone = new EventEmitter<StoryModel>();
  @Output() selected = new EventEmitter<StoryModel>();

  constructor() { }

  ngOnInit() {
  }

}
