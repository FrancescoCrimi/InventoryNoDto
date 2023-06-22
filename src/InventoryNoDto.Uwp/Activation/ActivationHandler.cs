// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using System.Threading.Tasks;

namespace Inventory.Uwp.Activation
{
    // Extend this class to implement new ActivationHandlers
    internal abstract class ActivationHandler<T> : IActivationHandler
        where T : class
    {
        // You can override this method to add extra validation on activation args
        // to determine if your ActivationHandler should handle this activation args
        protected virtual bool CanHandleInternal(T args) => true;

        // Override this method to add the activation logic in your activation handler
        protected abstract Task HandleInternalAsync(T args);

        public async Task HandleAsync(object args) =>  await HandleInternalAsync(args as T);

        // CanHandle checks the args is of type you have configured
        public bool CanHandle(object args) => args is T && CanHandleInternal(args as T);
    }
}
