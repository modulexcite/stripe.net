﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Stripe.Tests
{
    public class when_updating_a_dispute
    {
        private static StripeCharge _disputedCharge;
        private static StripeDispute _dispute;

        Establish context = () =>
        {
            var chargeService = new StripeChargeService();
            var disputedOptions = test_data.stripe_dispute_options.DisputedCard();

            var initialCharge = chargeService.Create(disputedOptions);

            // we need to wait for Stripe to process the dispute. Try for 10 seconds.
            var stopwatch = Stopwatch.StartNew();
            do
            {
                _disputedCharge = chargeService.Get(initialCharge.Id);
                if (_disputedCharge.Dispute != null) break;
                Thread.Sleep(500);
            } while (stopwatch.ElapsedMilliseconds < 10000);
        };

        Because of = () =>
        {
            _dispute = new StripeDisputeService().Update(_disputedCharge.Dispute.Id, new StripeDisputeUpdateOptions()
            {
                CustomerEmailAddress = "indiana@jones.com"
            });
        };

        It should_not_be_null = () =>
            _dispute.ShouldNotBeNull();

        It should_have_the_right_email = () =>
            _dispute.Evidence.CustomerEmailAddress.ShouldEqual("indiana@jones.com");
    }
}
