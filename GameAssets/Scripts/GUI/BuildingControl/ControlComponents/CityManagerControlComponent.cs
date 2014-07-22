using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CityManagerControlComponent : ControlComponent
{
    public BinaryLabelBar taxRate;
    public BinaryLabelBar population;
    public BinaryLabelBar wallet;

    CityManager _cityManager;

    void Start()
    {
        _cityManager = GetParentObjectComponent<CityManager>();
        SetDetails();

    }

    void Update()
    {
        SetDetails();
    }


    void SetDetails()
    {
        taxRate.SecondLabel.text = _cityManager.tax.ToString();
        population.SecondLabel.text = _cityManager.Citizens.Count.ToString();
        wallet.SecondLabel.text = _cityManager.Wallet.Currency.ToString();
    }





}

