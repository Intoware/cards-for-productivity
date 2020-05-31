import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-card-item',
  templateUrl: './card-item.component.html',
  styleUrls: ['./card-item.component.scss']
})
export class CardItemComponent implements OnInit {

  @Input() clickable: boolean;

  @Input() frontContent: string;
  @Input() backContent: string;

  @Input() showFront: boolean;

  @Input() frontColor: string;
  @Input() backColor: string;
  @Input() frontBackgroundColor: string;
  @Input() backBackgroundColor: string;
  @Input() frontBorder: string;
  @Input() backBorder: string;
  @Input() frontFontSize: string;
  @Input() backFontSize: string;

  @Input() width: string;
  @Input() height: string;

  constructor() { }

  ngOnInit(): void {
  }

}
