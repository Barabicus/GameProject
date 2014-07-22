using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class CurrencyContainer
{
    private int _currency;

    public int Currency
    {
        get { return _currency; }
        set { _currency = value; }
    }

    /// <summary>
    /// Attempts to transfer the specified amount from the supplied currency container. If the 
    /// other container does not have sufficient funds to transfer the transaction is considered failed
    /// and nothing will be transferred at all.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool TransferCurrency(CurrencyContainer other, int amount)
    {
        if (other.Currency >= amount)
        {
            Currency += amount;
            other.Currency -= amount;
            return true;
        }
        return false;
            
    }


}

