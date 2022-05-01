export class PortfolioModel {

  public PortfolioID: string = '';
  public PortfolioName: string = '';
  public PortfolioAssociations!: PortfolioAssociationsModel[];

}

export class PortfolioAssociationsModel {
  public AssociatedPortfolioID: string = '';
  public RelationShipDescription: string = '';
}
